module "eks" {
  source  = "terraform-aws-modules/eks/aws"
  version = "~> 20.24"

  cluster_name    = "${var.project_name}-eks"
  cluster_version = var.kubernetes_version

  cluster_endpoint_public_access = true

  # Grant the Terraform caller (your IAM user/role) cluster-admin so kubectl
  # works immediately after apply. Sufficient for a single-user POC.
  enable_cluster_creator_admin_permissions = true

  vpc_id = module.vpc.vpc_id
  # POC cost saving: nodes run in the public subnets (no NAT gateway needed).
  subnet_ids = module.vpc.public_subnets

  eks_managed_node_group_defaults = {
    ami_type = "AL2023_x86_64_STANDARD"
  }

  eks_managed_node_groups = {
    default = {
      instance_types = [var.node_instance_type]
      capacity_type  = var.node_capacity_type

      min_size     = 1
      max_size     = var.node_max_size
      desired_size = var.node_desired_size

      # Required so nodes in public subnets get a public IP for image pulls.
      subnet_ids = module.vpc.public_subnets
    }
  }
}
