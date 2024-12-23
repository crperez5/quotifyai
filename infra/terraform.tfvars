environment               = "dev"
resource_group_name       = "quotifyairg"
storage_account_name      = "quotifyaisa"
location                  = "West Europe"
azure_subscription_id     = "b01be5c4-2593-4dba-9bea-c484e74ec324"
azure_tenant_id           = "4d88b5e5-2d7e-44fd-a348-af5ee06e099a"
key_vault_name            = "quotifyaiva"
function_app_name         = "quotifyaifu"
service_plan_name         = "quotifyaisp"
application_insights_name = "quotifyaiapin"
function_tags = {
  "azd-service-name" = "function"
}
