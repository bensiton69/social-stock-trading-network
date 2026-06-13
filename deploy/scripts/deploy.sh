#!/usr/bin/env bash
# Configure kubeconfig, create the DB secret from Terraform output, and apply
# the Kubernetes manifests.
# Usage: deploy/scripts/deploy.sh [image-tag]   (default tag: latest)
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
REPO_ROOT="$(cd "${SCRIPT_DIR}/../.." && pwd)"
TF_DIR="${REPO_ROOT}/deploy/terraform"
K8S_DIR="${REPO_ROOT}/deploy/k8s"

TAG="${1:-latest}"
NAMESPACE="social-stocks"

REGION="$(terraform -chdir="${TF_DIR}" output -raw aws_region)"
CLUSTER="$(terraform -chdir="${TF_DIR}" output -raw cluster_name)"
ECR_URL="$(terraform -chdir="${TF_DIR}" output -raw ecr_repository_url)"
CONNECTION_STRING="$(terraform -chdir="${TF_DIR}" output -raw db_connection_string)"

echo "Updating kubeconfig for cluster ${CLUSTER} ..."
aws eks update-kubeconfig --region "${REGION}" --name "${CLUSTER}"

echo "Applying namespace ..."
kubectl apply -f "${K8S_DIR}/namespace.yaml"

echo "Creating/updating DB secret ..."
kubectl create secret generic socialstocks-db \
  --namespace "${NAMESPACE}" \
  --from-literal=connection-string="${CONNECTION_STRING}" \
  --dry-run=client -o yaml | kubectl apply -f -

echo "Deploying API (${ECR_URL}:${TAG}) ..."
sed "s#REPLACE_WITH_ECR_IMAGE#${ECR_URL}:${TAG}#g" "${K8S_DIR}/deployment.yaml" |
  kubectl apply -f -
kubectl apply -f "${K8S_DIR}/service.yaml"

echo "Waiting for rollout ..."
kubectl rollout status deployment/api -n "${NAMESPACE}" --timeout=240s

echo
echo "Service (the EXTERNAL-IP/hostname is the public NLB endpoint):"
kubectl get svc api -n "${NAMESPACE}"
