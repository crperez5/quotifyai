variable "environment" {
  type        = string
  description = "Environment name (e.g., dev, prod)"
}

variable "location" {
  type        = string
  description = "Azure region"
}

variable "resource_group_name" {
  type        = string
  description = "Name of the resource group"
}

variable "tags" {
  type        = map(string)
  description = "Resource tags"
  default     = {}
}

variable "cognitive_service_name" {
  type = string
}

variable "vnet_id" {
  type = string
}

variable "subnet_id" {
  type = string
}

variable "private_endpoint_name" {
  type = string
}

