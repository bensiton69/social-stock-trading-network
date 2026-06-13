output "aws_region" {
  description = "Region the POC is deployed to."
  value       = var.aws_region
}

output "cluster_name" {
  description = "EKS cluster name."
  value       = module.eks.cluster_name
}

output "update_kubeconfig_command" {
  description = "Run this to point kubectl at the cluster."
  value       = "aws eks update-kubeconfig --region ${var.aws_region} --name ${module.eks.cluster_name}"
}

output "ecr_repository_url" {
  description = "ECR repository URL for the API image."
  value       = aws_ecr_repository.api.repository_url
}

output "rds_endpoint" {
  description = "RDS PostgreSQL endpoint address."
  value       = aws_db_instance.this.address
}

output "db_secret_name" {
  description = "Secrets Manager secret holding the DB credentials/connection string."
  value       = aws_secretsmanager_secret.db.name
}

output "db_connection_string" {
  description = "Default connection string for the API (sensitive)."
  value       = local.db_connection_string
  sensitive   = true
}
