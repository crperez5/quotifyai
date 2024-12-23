variable "name" {
  description = "Container App name"
  type        = string
}

variable "resource_group_name" {
  description = "Resource Group name"
  type        = string
}

variable "location" {
  description = "Region to be used by resources"
  type        = string
  default     = "West Europe" 
} 

variable "container_registry_name" {
  description = "Container Registry name"
  type        = string
}