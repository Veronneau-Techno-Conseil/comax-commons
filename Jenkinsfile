def version = ''
def chartVersion = ''
def chartAction = ''
def patch = ''
def shouldUninstall = ''
def deployAction = ''
def message = ''
pipeline {
    agent any

    stages {
        stage('Prepare') {
            steps {
                sh 'echo "Build Summary: \n" > SUMMARY'
                script {
                    withCredentials([string(credentialsId: 'hangouts_token', variable: 'CHATS_TOKEN')]) {
                        hangoutsNotifyBuildStart token: "$CHATS_TOKEN",threadByJob: false
                    }
                    
                    // Get some code from a GitHub repository
                    git branch: '$BRANCH_NAME', url: 'https://github.com/Veronneau-Techno-Conseil/comax-accounts.git'
                
                    version = readFile('VERSION').trim()
                    chartVersion = readFile('./helm/VERSION').trim()
                    patch = version
                }
            }
        }
        stage('Node compile') {
            steps {
                nodejs(nodeJSInstallationName: 'NodeJs') {
                    sh 'cd "src/Accounts/" && npm install'
                    sh 'cd "src/Accounts/" && node ./browserify.js'
                    sh 'cd "src/Accounts/" && npx gulp min'
                }
            }
        }
        stage('Build') {
            steps {
                script {
                    def customImage = docker.build("registry.vtck3s.lan/comax-accounts:latest")
                    customImage.push()
                    customImage.push(patch)
                }
                sh 'echo "Build registry.vtck3s.lan/comax-accounts:${version} pushed to registry \n" >> SUMMARY'

                script {
                    def centralapi = docker.build("registry.vtck3s.lan/comax-central:latest", "-f ./central.Dockerfile .")
                    centralapi.push()
                    centralapi.push(patch)
                }
                sh 'echo "Build registry.vtck3s.lan/comax-central:${version} pushed to registry \n" >> SUMMARY'
            }

            post {
                success {
                    script {
                        currentBuild.displayName = version
                    }
                }                
            }
        }
        stage('Prep Helm') {
            steps {
                sh 'mkdir penv && python3 -m venv ./penv'
                sh '. penv/bin/activate && pwd && ls -l && pip install -r ./build/requirements.txt && python3 ./build/processchart.py'
                sh 'curl -k https://charts.vtck3s.lan/api/charts/comax-accounts/${chartVersion} | jq \'.name | "DEPLOY"\' > CHART_ACTION'
                script {
                    chartAction = readFile('CHART_ACTION').replace('"','').trim()
                }
            }
        }
        stage('Helm') {
            when{
                expression {
                    return chartAction == "DEPLOY"
                }
            }
            steps {
                withCredentials([file(credentialsId: 'pdsk3s', variable: 'kubecfg'), file(credentialsId: 'helmrepos', variable: 'repos')]) {
                    sh 'helm lint ./helm/'
                    sh 'helm repo update --repository-config ${repos}'
                    sh 'helm dependency update ./helm/ --repository-config ${repos}'
                    sh 'helm package ./helm/ --repository-config ${repos}'
                    sh 'CHARTVER=$(cat ./helm/VERSION) && curl -k --data-binary "@comax-accounts-$CHARTVER.tgz" https://charts.vtck3s.lan/api/charts'
                }
            }
        }
        stage('SKIP Helm') {
            when{
                expression {
                    return chartAction != "DEPLOY"
                }
            }
            steps {
                sh 'echo "Skipped helm chart deployment du to preexisting chart version ${chartVersion} \n" >> SUMMARY'
            }
        }
        stage('Prepare Application deployment') {
            when{
                expression {
                    return env.BRANCH_NAME.startsWith('release')
                }
            }
            steps {
                withCredentials([file(credentialsId: 'pdsk3s', variable: 'kubecfg'), file(credentialsId: 'helmrepos', variable: 'repos')]) {
                    sh 'helm repo update --repository-config ${repos}'
                    sh 'helm dependency update ./helm --repository-config ${repos}'
                    sh 'helm list -n cx-accounts --output=json --kubeconfig $kubecfg > HELM_LIST'
                    sh 'cat HELM_LIST'
                    sh 'jq \'select(.[].name == "accounts") | select(.[].status == "deployed") | "upgrade" \' HELM_LIST > DEPLOY_ACTION'
                    sh 'jq \'select(.[].name == "accounts") | select(.[].status != "deployed") | "uninstall" \' HELM_LIST > SHOULD_UNINSTALL'
                    sh 'cat DEPLOY_ACTION && cat SHOULD_UNINSTALL'
                    script {
                        deployAction = readFile('DEPLOY_ACTION').replace('"','').trim()
                        shouldUninstall = readFile('SHOULD_UNINSTALL').replace('"','').trim()
                    }
                    echo "Deploy action: ${deployAction}"
                    echo "Should uninstall: ${shouldUninstall}"
                }
            }
        }
        stage('Uninstall Application deployment') {
            when{
                expression {
                    return env.BRANCH_NAME.startsWith('release') && shouldUninstall == 'uninstall'
                }
            }
            steps {
                withCredentials([file(credentialsId: 'pdsk3s', variable: 'kubecfg'), file(credentialsId: 'helmrepos', variable: 'repos')]) {
                    sh 'helm -n cx-accounts uninstall accounts --kubeconfig ${kubecfg}'
                }
            }
        }
        stage('Install Application deployment') {
            when{
                expression {
                    return env.BRANCH_NAME.startsWith('release') && deployAction != "upgrade"
                }
            }
            steps {
                echo "Deploy action: ${deployAction}"
                withCredentials([file(credentialsId: 'pdsk3s', variable: 'kubecfg'), file(credentialsId: 'helmrepos', variable: 'repos'), file(credentialsId: 'accountsvalues', variable: 'accountsvalues')]) {
                    sh 'helm -n cx-accounts install accounts ./helm/ --kubeconfig ${kubecfg} --repository-config ${repos} -f ${accountsvalues}'
                }
            }
        }
        stage('Upgrade Application deployment') {
            when{
                expression {
                    return env.BRANCH_NAME.startsWith('release') && deployAction == "upgrade"
                }
            }
            steps {
                withCredentials([file(credentialsId: 'pdsk3s', variable: 'kubecfg'), file(credentialsId: 'helmrepos', variable: 'repos'), file(credentialsId: 'accountsvalues', variable: 'accountsvalues')]) {
                    sh 'helm -n cx-accounts upgrade accounts ./helm/ --kubeconfig ${kubecfg} --repository-config ${repos} -f ${accountsvalues}'
                }
            }
        }
        stage('Finalize') {
            steps {
                script {
                    message = readFile('SUMMARY')
                }
            }
        }
    }
    post {
        success {
            withCredentials([string(credentialsId: 'hangouts_token', variable: 'CHATS_TOKEN')]) {
                hangoutsNotify message: message, token: "$CHATS_TOKEN", threadByJob: false
                hangoutsNotifySuccess token: "$CHATS_TOKEN", threadByJob: false
            }
        }
        failure {
            withCredentials([string(credentialsId: 'hangouts_token', variable: 'CHATS_TOKEN')]) {
                hangoutsNotify message: message, token: "$CHATS_TOKEN", threadByJob: false
                hangoutsNotifyFailure token: "$CHATS_TOKEN", threadByJob: false
            }
        }
        always {
            script {
                cleanWs()
            }
        }
    }
}
