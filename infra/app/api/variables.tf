variable "resource_group_name" {
  description = "Resource Group name"
  type        = string
}

variable "api_name" {
  description = "API name"
  type        = string
}

variable "container_registry_url" {
  description = "Container Registry url"
  type        = string
}

variable "image_name" {
  description = "The container image name for the app"
  type        = string
}

variable "user_identity_id" {
  description = "The Usser Assigned identity to use for the app"
  type        = string
}

variable "container_apps_environment_id" {
  description = "The Container Apps Environment id"
  type        = string
}

variable "tags" {
  description = "List of tags to assign to the function"
  type        = map(string)
}

variable "env" {
  description = "List of env variables"
  type = list(object({
    name  = string
    value = string
  }))
}
