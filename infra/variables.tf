variable "resource_group_name" {
  description = "Resource Group name"
  type        = string
}

variable "location" {
  description = "Region to be used by resources"
  type        = string
  default     = "West Europe" 
}

variable "azure_subscription_id" {
  description = "What Azure subscription should the workload identity have access to?"
  type        = string
}

variable "azure_tenant_id" {
  type = string
}