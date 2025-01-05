variable "azure_tenant_id" {
  type = string
}

variable "azure_subscription_id" {
  description = "What Azure subscription should the workload identity have access to?"
  type        = string
}

variable "location" {
  description = "Region to be used by resources"
  type        = string
  default     = "West Europe"
}

variable "ai_location" {
  description = "Region to be used by ai resources"
  type        = string
  default     = "swedencentral"
}

variable "environment" {
  type = string
}

variable "tags" {
  description = "List of tags"
  type        = map(string)
}

variable "apps_resource_group_name" {
  description = "Resource Group name for apps"
  type        = string
}

variable "ai_resource_group_name" {
  description = "Resource Group name for AI resources"
  type        = string
}

variable "storage_account_name" {
  description = "Storage Account name"
  type        = string
}

variable "container_app_environment_storage_name" {
  type = string
}

variable "storage_account_container" {
  description = "Storage Account container"
  type        = string
}

variable "key_vault_name" {
  type        = string
  description = "Name of the key vault"
}

variable "service_plan_name" {
  description = "Service Plan name"
  type        = string
}

variable "function_app_name" {
  description = "Function App name"
  type        = string
}

variable "application_insights_name" {
  description = "App Insights name"
  type        = string
}

variable "log_analytics_name" {
  description = "Log Analytics name"
  type        = string
}

variable "container_registry_name" {
  description = "Container Registry name"
  type        = string
}

variable "container_apps_environment_name" {
  type = string
}

variable "api_name" {
  description = "API name"
  type        = string
}

variable "app_identity_name" {
  description = "app identity name"
  type        = string
}

variable "vector_store_name" {
  description = "vector store name"
  type        = string
}

variable "vector_store_image_name" {
  type = string
}

variable "vector_store_volume_mount_name" {
  type = string
}

variable "vector_store_volume_mount_path" {
  type = string
}

variable "vnet_name" {
  type = string
}

variable "cognitive_service_name" {
  type = string
}