variable "environment" {
  type = string
}

variable "cognitive_service_name" {
  type = string
}

variable "ai_resource_group_name" {
  description = "Resource Group name that hosts AI resources"
  type        = string
}

variable "apps_resource_group_name" {
  description = "Resource Group name that hosts app resources"
  type        = string
}

variable "ai_location" {
  description = "Region to be used by ai resources"
  type        = string
  default     = "swedencentral"
}

variable "apps_location" {
  description = "Region to be used by app resources"
  type        = string
  default     = "westeurope"
}

variable "apps_vnet_name" {
  type = string
}

variable "apps_vnet_id" {
  type = string
}

variable "tags" {
  description = "List of tags"
  type        = map(string)
}
