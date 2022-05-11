import { AccountInterface, AccountModel } from './Models/authorize-model';
import { HttpClient, HttpParams, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { OperationResult } from '../contracts/OperationResult';

@Injectable({
  providedIn: 'root'
})
export class AuthorizeServiceService {
  readonly CommonsURL = "";

  constructor(private http: HttpClient) { }

  AuthenticateCluster(account: AccountModel): Observable<OperationResult<{token: string}>> {
    
    return this.http.post(this.CommonsURL + "api/authentication/cluster", account);
  }

  AuthenticateUser(): Observable<OperationResult<{token: string}>>{
    return this.http.post(this.CommonsURL + "api/authentication", {});
  }

  Init(): Observable<OperationResult<string>>{
    return this.http.get<OperationResult<string>>(this.CommonsURL + "api/authentication");
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
