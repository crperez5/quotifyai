variable "app_name" {
  description = "App name"
  type        = string
}

variable "app_id" {
  description = "App id"
  type        = string
}


variable "resource_group_name" {
  description = "Resource Group name"
  type        = string
}

variable "image_name" {
  description = "Container image"
  type        = string
}

variable "container_registry_url" {
  description = "Container registry url"
  type        = string
}

variable "user_identity_id" {
  description = "The Identity ID to use"
  type        = string
}

variable "container_apps_environment_id" {
  type = string
}

variable "tags" {
  description = "List of tags"
  type        = map(string)
}