export class AccountModel {
  public ApplicationID: string = "";
  public ClientID: string = "";
  public ClientSecret: string = "";
  public UserChallengeCode: string = "";
  public AccessToken: string = "";
}

export interface AccountInterface {
  application_id: string;
  client_id: string;
  client_secret: string;
  access_token: string;
  token_type: string,
  expires_in: number,
  scope: string,
  id_token: string
}
