variable "resource_group_name" {
  description = "Resource Group name"
  type        = string
}

variable "location" {
  description = "Region to be used by resources"
  type        = string
  default     = "westeurope"
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

variable "infrastructure_subnet_id" {
  type = string
}

variable "peer_subnet_id" {
  description = "Specify a subnet id to peer with to allow traffic in and out"
  type        = string
  default     = null
}

variable "infrastructure_subnet_address_prefix" {
  type    = string
  default = null
}

variable "peer_subnet_address_prefix" {
  type    = string
  default = null
}

variable "env" {
  description = "List of env variables"
  type = list(object({
    name  = string
    value = string
  }))
}
