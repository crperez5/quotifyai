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
  type = string
}

variable "container_registry_name" {
  description = "Container Registry name"
  type        = string
}

variable "container_apps_environment_name" {
  type = string
}

variable "log_analytics_workspace_id" {
  type = string
}

variable "tags" {
  description = "List of tags"
  type        = map(string)
}

variable "user_identity_id" {
  description = "User Identity to assign to the function"
  type        = string
}

variable "storage_account_name" {
  type = string
}

variable "storage_account_access_key" {
  type = string
  sensitive = true
}

variable "file_share_name" {
  type = string
}

variable "container_app_environment_storage_name" {
  type = string
}