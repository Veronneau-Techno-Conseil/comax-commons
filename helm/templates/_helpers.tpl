{{/*
Expand the name of the secret.
*/}}
{{- define "comax-orchestrator.secretName" -}}
{{- printf "%s-secrets" ( include "comax-orchestrator.name" . ) -}}
{{- end }}


{{/*
Expand the full service name.
*/}}
{{- define "comax-orchestrator.fullSvcName" -}}
{{- printf "%s.%s.svc.cluster.local" ( include "comax-orchestrator.fullname" . ) .Release.Namespace -}}
{{- end }}

{{/*
Expand the full service name.
*/}}
{{- define "comax-mariadb.fullSvcName" -}}
{{- printf "%s.%s.svc.cluster.local" ( include "comax-mariadb.fullname" . ) .Release.Namespace -}}
{{- end }}

{{/*
Expand the full service name.
*/}}
{{- define "comax-grainstorage.fullSvcName" -}}
{{- printf "%s.%s.svc.cluster.local" ( include "comax-grainstorage.fullname" . ) .Release.Namespace -}}
{{- end }}

{{/*
Expand the name of the chart.
*/}}
{{- define "comax-orchestrator.name" -}}
{{- default .Chart.Name .Values.nameOverride | trunc 63 | trimSuffix "-" }}
{{- end }}

{{/*
Expand the name of the chart.
*/}}
{{- define "comax-grainstorage.name" -}}
{{- printf "%s-gs" (default .Chart.Name .Values.nameOverride | trunc 63 | trimSuffix "-") }}
{{- end }}

{{/*
Expand the name of the chart.
*/}}
{{- define "comax-mariadb.name" -}}
{{- printf "%s-gsdb" (default .Chart.Name .Values.nameOverride | trunc 63 | trimSuffix "-") }}
{{- end }}


{{/*
Create a default fully qualified app name.
We truncate at 63 chars because some Kubernetes name fields are limited to this (by the DNS naming spec).
If release name contains chart name it will be used as a full name.
*/}}
{{- define "comax-grainstorage.fullname" -}} 
{{- $name := (include "comax-grainstorage.name" . ) }}
{{- if contains $name .Release.Name }}
{{- .Release.Name | trunc 63 | trimSuffix "-" }}
{{- else }}
{{- printf "%s-%s" .Release.Name $name | trunc 63 | trimSuffix "-" }}
{{- end }}
{{- end }}

{{/*
Create a default fully qualified app name.
We truncate at 63 chars because some Kubernetes name fields are limited to this (by the DNS naming spec).
If release name contains chart name it will be used as a full name.
*/}}
{{- define "comax-mariadb.fullname" -}} 
{{- $name := (include "comax-mariadb.name" . ) }}
{{- if contains $name .Release.Name }}
{{- .Release.Name | trunc 63 | trimSuffix "-" }}
{{- else }}
{{- printf "%s-%s" .Release.Name $name | trunc 63 | trimSuffix "-" }}
{{- end }}
{{- end }}



{{/*
Create a default fully qualified app name.
We truncate at 63 chars because some Kubernetes name fields are limited to this (by the DNS naming spec).
If release name contains chart name it will be used as a full name.
*/}}
{{- define "comax-orchestrator.fullname" -}} 
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
{{- define "comax-orchestrator.chart" -}}
{{- printf "%s-%s" .Chart.Name .Chart.Version | replace "+" "_" | trunc 63 | trimSuffix "-" }}
{{- end }}

{{/*
Common labels
*/}}
{{- define "comax-orchestrator.labels" -}}
helm.sh/chart: {{ include "comax-orchestrator.chart" . }}
{{ include "comax-orchestrator.selectorLabels" . }}
{{- if .Chart.AppVersion }}
app.kubernetes.io/version: {{ .Chart.AppVersion | quote }}
{{- end }}
app.kubernetes.io/managed-by: {{ .Release.Service }}
{{- end }}

{{/*
Common labels
*/}}
{{- define "comax-grainstorage.labels" -}}
helm.sh/chart: {{ include "comax-orchestrator.chart" . }}
{{ include "comax-grainstorage.selectorLabels" . }}
{{- if .Chart.AppVersion }}
app.kubernetes.io/version: {{ .Chart.AppVersion | quote }}
{{- end }}
app.kubernetes.io/managed-by: {{ .Release.Service }}
{{- end }}

{{/*
Common labels
*/}}
{{- define "comax-mariadb.labels" -}}
helm.sh/chart: {{ include "comax-orchestrator.chart" . }}
{{ include "comax-mariadb.selectorLabels" . }}
{{- if .Chart.AppVersion }}
app.kubernetes.io/version: {{ .Chart.AppVersion | quote }}
{{- end }}
app.kubernetes.io/managed-by: {{ .Release.Service }}
{{- end }}

{{/*
Selector labels
*/}}
{{- define "comax-grainstorage.selectorLabels" -}}
app.kubernetes.io/name: {{ include "comax-grainstorage.name" . }}
app.kubernetes.io/instance: {{ .Release.Name }}
{{- end }}

{{/*
Selector labels
*/}}
{{- define "comax-mariadb.selectorLabels" -}}
app.kubernetes.io/name: {{ include "comax-mariadb.name" . }}
app.kubernetes.io/instance: {{ .Release.Name }}
{{- end }}

{{/*
Selector labels
*/}}
{{- define "comax-orchestrator.selectorLabels" -}}
app.kubernetes.io/name: {{ include "comax-orchestrator.name" . }}
app.kubernetes.io/instance: {{ .Release.Name }}
{{- end }}

{{/*
Create the name of the service account to use
*/}}
{{- define "comax-orchestrator.serviceAccountName" -}}
{{- if .Values.serviceAccount.create }}
{{- default (include "comax-orchestrator.fullname" .) .Values.serviceAccount.name }}
{{- else }}
{{- default "default" .Values.serviceAccount.name }}
{{- end }}
{{- end }}

