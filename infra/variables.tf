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

variable "environment" {
  type = string
}

variable "resource_group_name" {
  description = "Resource Group name"
  type        = string
}

variable "storage_account_name" {
  description = "Storage Account name"
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

variable "function_tags" {
  description = "List of tags to assign to the function"
  type        = map(string)
}

variable "application_insights_name" {
  description = "App Insights name"
  type        = string
}
