resource "azuread_application_federated_identity_credential" "github_federated_credentials" {
  application_id        = "5b3dfe23-3d46-45a4-9a9f-2309e958ccde"
  display_name          = "githubagent"
  subject               = "repo:crperez5/quotifyai:ref:refs/heads/main"
  audiences             = ["api://AzureADTokenExchange"]
  issuer                = "https://token.actions.githubusercontent.com"
}

output "federated_identity_credential_id" {
  value = azuread_application_federated_identity_credential.github_federated_credentials.id
}
