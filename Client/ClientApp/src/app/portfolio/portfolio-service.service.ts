import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { PortfolioModel } from './Models/portfolio-model';

@Injectable({
  providedIn: 'root'
})
export class PortfolioServiceService {

  readonly CommonsURL = "https://localhost:44369/";

  constructor(private http: HttpClient) { }

  CreatePortfolio(portfolio: PortfolioModel): Observable<any> {
    let params = new HttpParams()
      .set('ID', portfolio.ID)
      .set('name', portfolio.Name)
      .set('type', portfolio.Type)
      .set('parentId', portfolio.ParentId);
    return this.http.post(this.CommonsURL + "api/portfolio/create/" + portfolio.ID, portfolio)
  }
}
