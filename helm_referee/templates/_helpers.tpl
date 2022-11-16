{{/*
Expand the name of the secret.
*/}}
{{- define "comax-referee.secretName" -}}
{{- printf "%s-secrets" ( include "comax-referee.name" . ) -}}
{{- end }}


{{/*
Expand the full service name.
*/}}
{{- define "comax-referee.fullSvcName" -}}
{{- printf "%s.%s.svc.cluster.local" ( include "comax-referee.name" . ) .Release.Namespace -}}
{{- end }}

{{/*
Expand the full service name.
*/}}
{{- define "comax-referee-mongodb.fullSvcName" -}}
{{- printf "referee-mongodb.%s.svc.cluster.local" .Release.Namespace -}}
{{- end }}

{{/*
Expand the name of the cert secret.
*/}}
{{- define "comax-referee.certSecretName" -}}
{{- printf "%s-depltls" ( include "comax-referee.name" . ) -}}
{{- end }}

{{/*
Expand the name of the chart.
*/}}
{{- define "comax-referee.name" -}}
{{- default .Chart.Name .Values.nameOverride | trunc 63 | trimSuffix "-" }}
{{- end }}


{{/*
Create a default fully qualified app name.
We truncate at 63 chars because some Kubernetes name fields are limited to this (by the DNS naming spec).
If release name contains chart name it will be used as a full name.
*/}}
{{- define "comax-referee.fullname" -}} 
{{- if .Values.fullnameOverride }}
{{- .Values.fullnameOverride | trunc 63 | trimSuffix "-" }}
{{- else }}
{{- $name := default .Chart.Name .Values.nameOverride }}
{{- if contains $name .Release.Name }}
{{- .Release.Name | trunc 63 | trimSuffix "-" }}
{{- else }}
{{- printf "%s-%s" .Release.Name $name | trunc 63 | trimSuffix "-" }}
{{- end }}
{{- end }}
{{- end }}

{{/*
Create chart name and version as used by the chart label.
*/}}
{{- define "comax-referee.chart" -}}
{{- printf "%s-%s" .Chart.Name .Chart.Version | replace "+" "_" | trunc 63 | trimSuffix "-" }}
{{- end }}

{{/*
Common labels
*/}}
{{- define "comax-referee.labels" -}}
helm.sh/chart: {{ include "comax-referee.chart" . }}
{{ include "comax-referee.selectorLabels" . }}
{{- if .Chart.AppVersion }}
app.kubernetes.io/version: {{ .Chart.AppVersion | quote }}
{{- end }}
app.kubernetes.io/managed-by: {{ .Release.Service }}
{{- end }}


{{/*
Selector labels
*/}}
{{- define "comax-referee.selectorLabels" -}}
app.kubernetes.io/name: {{ include "comax-referee.name" . }}
app.kubernetes.io/instance: {{ .Release.Name }}
{{- end }}

{{/*
Create the name of the service account to use
*/}}
{{- define "comax-referee.serviceAccountName" -}}
{{- if .Values.serviceAccount.create }}
{{- default (include "comax-referee.fullname" .) .Values.serviceAccount.name }}
{{- else }}
{{- default "default" .Values.serviceAccount.name }}
{{- end }}
{{- end }}

