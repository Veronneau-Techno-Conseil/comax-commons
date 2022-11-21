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
                    git branch: '$BRANCH_NAME', url: 'https://github.com/Veronneau-Techno-Conseil/comax-commons.git'
                
                    version = readFile('VERSION').trim()
                    chartVersion = readFile('./helm/VERSION').trim()
                    patch = version
                }
            }
        }
        
        stage('Build') {
            steps {
                
                script {
                    def orch = docker.build("registry.vtck3s.lan/comax-orchestrator:latest", "-f ./orchestrator.Dockerfile .")
                    orch.push()
                    orch.push(patch)
                }
                sh 'echo "Build registry.vtck3s.lan/comax-orchestrator:${patch} pushed to registry \n" >> SUMMARY'

				sh 'docker buildx build -t registry.vtck3s.lan/comax-orchestrator:${patch}-arm64 --platform linux/arm64 -f orchestrator.Dockerfile .'
                sh 'docker tag registry.vtck3s.lan/comax-orchestrator:latest-arm64 registry.vtck3s.lan/comax-orchestrator:${patch}-arm64'
				
                script {
                    def customImage = docker.build("registry.vtck3s.lan/comax-referee:latest", "-f ./referee.Dockerfile .")
                    customImage.push()
                    customImage.push(patch)
                }
                sh 'echo "Build registry.vtck3s.lan/comax-referee:${patch} pushed to registry \n" >> SUMMARY'
            
			}

            post {
                success {
                    script {
                        currentBuild.displayName = version
                    }
                }                
            }
        }
        stage('Prep Helm Orchestrator') {
            steps {
                sh 'mkdir penv && python3 -m venv ./penv'
                sh '. penv/bin/activate && pwd && ls -l && pip install -r ./build/requirements.txt && python3 ./build/processchart.py'
                sh 'curl -k https://charts.vtck3s.lan/api/charts/comax-orchestrator/${chartVersion} | jq \'.name | "DEPLOY"\' > CHART_ACTION'
                script {
                    chartAction = readFile('CHART_ACTION').replace('"','').trim()
                }
            }
        }
        stage('Helm Orchestrator') {
            when{
                expression {
                    return chartAction == "DEPLOY"
                }
            }
            steps {
                withCredentials([file(credentialsId: 'edgek3s', variable: 'kubecfg'), file(credentialsId: 'helmrepos', variable: 'repos')]) {
                    sh 'helm lint ./helm/'
                    sh 'helm repo update --repository-config ${repos}'
                    sh 'helm dependency update ./helm/ --repository-config ${repos}'
                    sh 'helm package ./helm/ --repository-config ${repos}'
                    sh 'CHARTVER=$(cat ./helm/VERSION) && curl -k --data-binary "@comax-orchestrator-$CHARTVER.tgz" https://charts.vtck3s.lan/api/charts'
                }
            }
        }
        stage('SKIP Helm Orchestrator') {
            when{
                expression {
                    return chartAction != "DEPLOY"
                }
            }
            steps {
                sh 'echo "Skipped helm chart deployment du to preexisting chart version ${chartVersion} \n" >> SUMMARY'
            }
        }
        stage('Prepare Application deployment Orchestrator') {
            when{
                expression {
                    return env.BRANCH_NAME.startsWith('release')
                }
            }
            steps {
                withCredentials([file(credentialsId: 'edgek3s', variable: 'kubecfg'), file(credentialsId: 'helmrepos', variable: 'repos')]) {
                    sh 'helm repo update --repository-config ${repos}'
                    sh 'helm dependency update ./helm --repository-config ${repos}'
                    sh 'helm list -n comax-commons --output=json --kubeconfig $kubecfg > HELM_LIST'
                    sh 'cat HELM_LIST'
                    sh 'jq \'select(.[].name == "orchestrator") | select(.[].status == "deployed") | "upgrade" \' HELM_LIST > DEPLOY_ACTION'
                    sh 'jq \'select(.[].name == "orchestrator") | select(.[].status != "deployed") | "uninstall" \' HELM_LIST > SHOULD_UNINSTALL'
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
        stage('Uninstall Application deployment Orchestrator') {
            when{
                expression {
                    return env.BRANCH_NAME.startsWith('release') && shouldUninstall == 'uninstall'
                }
            }
            steps {
                withCredentials([file(credentialsId: 'edgek3s', variable: 'kubecfg'), file(credentialsId: 'helmrepos', variable: 'repos')]) {
                    sh 'helm -n comax-commons uninstall orchestrator --kubeconfig ${kubecfg}'
                }
            }
        }
        stage('Install Application deployment Orchestrator') {
            when{
                expression {
                    return env.BRANCH_NAME.startsWith('release') && deployAction != "upgrade"
                }
            }
            steps {
                echo "Deploy action: ${deployAction}"
                withCredentials([file(credentialsId: 'edgek3s', variable: 'kubecfg'), file(credentialsId: 'helmrepos', variable: 'repos'), file(credentialsId: 'orchvalues', variable: 'orchvalues')]) {
                    sh 'helm -n comax-commons install orchestrator ./helm/ --kubeconfig ${kubecfg} --repository-config ${repos} -f ${orchvalues}'
                }
            }
        }
        stage('Upgrade Application deployment Orchestrator') {
            when{
                expression {
                    return env.BRANCH_NAME.startsWith('release') && deployAction == "upgrade"
                }
            }
            steps {
                withCredentials([file(credentialsId: 'edgek3s', variable: 'kubecfg'), file(credentialsId: 'helmrepos', variable: 'repos'), file(credentialsId: 'orchvalues', variable: 'orchvalues')]) {
                    sh 'helm -n comax-commons upgrade orchestrator ./helm/ --kubeconfig ${kubecfg} --repository-config ${repos} -f ${orchvalues}'
                }
            }
        }
		
		
        stage('Prep Helm Referee') {
            steps {
                
                sh 'curl -k https://charts.vtck3s.lan/api/charts/comax-referee/${chartVersion} | jq \'.name | "DEPLOY"\' > CHART_ACTION'
                script {
                    chartAction = readFile('CHART_ACTION').replace('"','').trim()
                }
            }
        }
        stage('Helm Referee') {
            when{
                expression {
                    return chartAction == "DEPLOY"
                }
            }
            steps {
                withCredentials([file(credentialsId: 'pdsk3s', variable: 'kubecfg'), file(credentialsId: 'helmrepos', variable: 'repos')]) {
                    sh 'helm lint ./helm_referee/'
                    sh 'helm repo update --repository-config ${repos}'
                    sh 'helm dependency update ./helm_referee/ --repository-config ${repos}'
                    sh 'helm package ./helm_referee/ --repository-config ${repos}'
                    sh 'CHARTVER=$(cat ./helm_referee/VERSION) && curl -k --data-binary "@comax-referee-$CHARTVER.tgz" https://charts.vtck3s.lan/api/charts'
                }
            }
        }
        stage('SKIP Helm Referee') {
            when{
                expression {
                    return chartAction != "DEPLOY"
                }
            }
            steps {
                sh 'echo "Skipped helm chart deployment du to preexisting chart version ${chartVersion} \n" >> SUMMARY'
            }
        }
        stage('Prepare Application deployment Referee') {
            when{
                expression {
                    return env.BRANCH_NAME.startsWith('release')
                }
            }
            steps {
                withCredentials([file(credentialsId: 'pdsk3s', variable: 'kubecfg'), file(credentialsId: 'helmrepos', variable: 'repos')]) {
                    sh 'helm repo update --repository-config ${repos}'
                    sh 'helm dependency update ./helm_referee --repository-config ${repos}'
                    sh 'helm list -n cx-referee --output=json --kubeconfig $kubecfg > HELM_LIST'
                    sh 'cat HELM_LIST'
                    sh 'jq \'select(.[].name == "referee") | select(.[].status == "deployed") | "upgrade" \' HELM_LIST > DEPLOY_ACTION'
                    sh 'jq \'select(.[].name == "referee") | select(.[].status != "deployed") | "uninstall" \' HELM_LIST > SHOULD_UNINSTALL'
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
        stage('Uninstall Application deployment Referee') {
            when{
                expression {
                    return env.BRANCH_NAME.startsWith('release') && shouldUninstall == 'uninstall'
                }
            }
            steps {
                withCredentials([file(credentialsId: 'pdsk3s', variable: 'kubecfg'), file(credentialsId: 'helmrepos', variable: 'repos')]) {
                    sh 'helm -n cx-referee uninstall referee --kubeconfig ${kubecfg}'
                }
            }
        }
        stage('Install Application deployment Referee') {
            when{
                expression {
                    return env.BRANCH_NAME.startsWith('release') && deployAction != "upgrade"
                }
            }
            steps {
                echo "Deploy action: ${deployAction}"
                withCredentials([file(credentialsId: 'pdsk3s', variable: 'kubecfg'), file(credentialsId: 'helmrepos', variable: 'repos'), file(credentialsId: 'refereevalues', variable: 'refereevalues')]) {
                    sh 'helm -n cx-referee install referee ./helm_referee/ --kubeconfig ${kubecfg} --repository-config ${repos} -f ${refereevalues}'
                }
            }
        }
        stage('Upgrade Application deployment Referee') {
            when{
                expression {
                    return env.BRANCH_NAME.startsWith('release') && deployAction == "upgrade"
                }
            }
            steps {
                withCredentials([file(credentialsId: 'pdsk3s', variable: 'kubecfg'), file(credentialsId: 'helmrepos', variable: 'repos'), file(credentialsId: 'refereevalues', variable: 'refereevalues')]) {
                    sh 'helm -n cx-referee upgrade referee ./helm_referee/ --kubeconfig ${kubecfg} --repository-config ${repos} -f ${refereevalues}'
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
