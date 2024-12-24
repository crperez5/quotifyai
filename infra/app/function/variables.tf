variable "resource_group_name" {
  description = "Resource Group name"
  type        = string
}

variable "location" {
  description = "Region to be used by resources"
  type        = string
  default     = "West Europe" 
}

variable "service_plan_name" {
  description = "Service Plan name"
  type        = string
}

variable "storage_account_name" {
  description = "Storage Account name"
  type        = string
}

variable "storage_account_access_key" {
  description = "Storage Account access key"
  type        = string
}

variable "function_app_name" {
  description = "Function App name"
  type        = string
}

variable "application_insights_instrumentation_key" {
  description = "App Insights instrumentation key"
  type        = string
}

variable "application_insights_connection_string" {
  description = "App Insights connection string"
  type        = string
}

variable "user_identity_id" {
  description = "User Identity to assign to the function"
  type        = string
}

variable "tags" {
  description = "List of tags to assign to the function"
  type        = map(string)
}
