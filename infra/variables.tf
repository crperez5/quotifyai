variable "resource_group_name" {
  description = "Resource Group name"
  type        = string
}

variable "location" {
  description = "Region to be used by resources"
  type        = string
  default     = "West Europe" 
}

variable "environment" {
  type    = string
  default = "dev"
}

variable "deployment_user_name" {
  description = "deployment user name"
  type        = string
  default     = "github"    
}

variable "owner_role_name" {
  type        = string
  default     = "Owner"
  description = "The name of the Owner role given to the user-assigned identity"
}

variable "github_repository" {
  type        = string
  default     = "quotifyai"
  description = "The name of the GitHub repository to target"
}

variable "storage_account_name" {
  type        = string
  description = "The name of the storage account"
}

variable "account_replication_type" {
  type        = string
  description = "The Replication Type to use for this storage account"
  default     = "LRS"
}

variable "account_tier" {
  type        = string
  description = "The Tier to use for this storage account"
  default     = "Standard"
}

variable "github_organization_target" {
  type        = string
}

variable "tags" {
  description = "A mapping of tags to assign to the resources."
  type        = map(string)
}

variable "container_name" {
  type        = string
  description = "The name of the storage container"
  default     = "tfstate"
}

variable "automatic_container_name" {
  type        = string
  description = "The name of the storage container for AKS automatic"
}
