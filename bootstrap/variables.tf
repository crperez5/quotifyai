variable "location" {
  description = "Region to be used by resources"
  type        = string
  default     = "West Europe" 
}

variable "azure_env_name" {
  description = "The environment"
  type        = string
  default     = "dev" 
}

variable "github_organization_name" {
  description = "GitHub organization (or user)"
  type        = string
}

variable "azure_tenant_id" {
  type = string
}

variable "azure_subscription_id" {
  description = "What Azure subscription should the workload identity have access to?"
  type        = string
}

variable "github_token" {
  description = "GitHub admin token"
  type        = string
}

variable "github_repository_name" {
  description = "The GitHub repository to set up workload identity for"
  type        = string
}

variable "azure_roles" {
  description = "Which roles to assign to the workload identity in Azure?"
  type        = list(string)
  default     = ["Contributor", "User Access Administrator"]
}

variable "branches" {
  description = "List of git branches to add as subject identifiers"
  type        = list(string)
  default     = ["main"]
}

variable "tags" {
  description = "List of git tags to add as subject identifiers"
  type        = list(string)
  default     = []
}

variable "environments" {
  description = "List of GitHub environments to add as subject identifiers"
  type        = list(string)
  default     = ["dev"]
}

variable "pull_request" {
  description = "Add the 'pull request' subject identifier?"
  type        = bool
  default     = false
}

variable "rs_storage_account" {
  description = "tfstate storage account"
  type        = string
}

variable "rs_resource_group" {
  description = "tfstate resource group"
  type        = string
}

variable "rs_container_name" {
  description = "tfstate container name"
  type        = string
}

variable "azure_container_registry_url" {
  description = "Azure Container Registry url"
  type        = string
}

variable "azure_resource_group" {
  description = "Azure Resource Group"
  type        = string
}


 
