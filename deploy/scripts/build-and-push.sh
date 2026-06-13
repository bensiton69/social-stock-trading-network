#!/usr/bin/env bash
# Build the API container image and push it to the ECR repo created by Terraform.
# Usage: deploy/scripts/build-and-push.sh [image-tag]   (default tag: latest)
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
REPO_ROOT="$(cd "${SCRIPT_DIR}/../.." && pwd)"
TF_DIR="${REPO_ROOT}/deploy/terraform"

TAG="${1:-latest}"

REGION="$(terraform -chdir="${TF_DIR}" output -raw aws_region)"
ECR_URL="$(terraform -chdir="${TF_DIR}" output -raw ecr_repository_url)"
REGISTRY="${ECR_URL%/*}"

echo "Logging in to ECR registry ${REGISTRY} ..."
aws ecr get-login-password --region "${REGION}" |
  docker login --username AWS --password-stdin "${REGISTRY}"

echo "Building ${ECR_URL}:${TAG} (linux/amd64) ..."
# --platform ensures the image matches x86_64 EKS nodes even when built on
# an Apple Silicon / arm64 workstation.
docker build \
  --platform linux/amd64 \
  -f "${REPO_ROOT}/SocialStockTradingNetwork.Api/Dockerfile" \
  -t "${ECR_URL}:${TAG}" \
  "${REPO_ROOT}"

echo "Pushing ${ECR_URL}:${TAG} ..."
docker push "${ECR_URL}:${TAG}"

echo "Done. Image: ${ECR_URL}:${TAG}"
