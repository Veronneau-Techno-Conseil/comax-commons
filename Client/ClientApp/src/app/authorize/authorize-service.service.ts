import { AccountInterface, AccountModel } from './Models/authorize-model';
import { HttpClient, HttpParams, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AuthorizeServiceService {

  readonly AccountsURL = "https://localhost:5001/";
  readonly CommonsURL = "https://localhost:44369/";

  constructor(private http: HttpClient) { }

  RequestAuthorization(account: AccountModel): Observable<any> {
    let params = new HttpParams()
      .set('client_id', account.ClientID)
      .set('response_type', "code")
      .set('scope', "openid")
      .set('redirect_uri', (this.CommonsURL + "authorize/token")) //consider replacing with form value?!
      .set('nonce', "abcabc");
    return this.http.get(this.AccountsURL + "connect/authorize", { params, responseType: 'text' });
  }

  GetToken(Account: AccountModel): Observable<AccountInterface> {
    const headers = new HttpHeaders({ 'Content-Type': 'application/x-www-form-urlencoded' });
    const body = new HttpParams()
      .set('grant_type', 'authorization_code')
      .set('client_id', Account.ClientID)
      .set('client_secret', Account.ClientSecret)
      .set('redirect_uri', (this.CommonsURL + 'authorize/token')) //consider replacing with form value?!
      .set('code', Account.UserChallengeCode);
    return this.http.post<AccountInterface>(this.AccountsURL + "connect/token", body, { headers: headers });
  }

  //In the function below, the Grain associated to the account will be activated
  //The clientId will be used as an Id for the Grain
  //The ApplicationId can not be retrieved since it is not an OpenIdConnect field
  //The Id filed has been added by us by overriding the initial OpenId Table
  //Retreiveing the Id may need overriding the "GetOpenIddictServerRequest()" function in
  //Accounts -> AuthorizationController -> Authorize
  //As an option, we can add a timestamp to the ClientId so we get a definitely unique GrainId
  //as done in the below request
  SetDetails(account: AccountModel): Observable<any> {
    return this.http.post(this.CommonsURL + "api/account/SetDetails/" + account.ClientID + Date.now(), account);
  }
}
