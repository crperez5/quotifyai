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

variable "virtual_network_name" {
  description = "Vnet name"
}

variable "subnet_name" {
  description = "subnet name"
}

variable "address_space" {
  type    = list(string)
  default = ["40.0.0.0/16"]
}

variable "subnet_address_prefix_map" {
  type = map(list(string))
  default = {
    "app" = ["40.0.0.0/23"]
  }
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
