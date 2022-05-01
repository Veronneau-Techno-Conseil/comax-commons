import { Component, NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AuthorizeComponent } from './authorize/authorize.component';
import { TestGrainComponent } from './test-grain/test-grain.component';
import { TokenComponent } from './authorize/token/token.component';
import { PortfolioComponent } from './portfolio/portfolio.component';
import { CreatePortfolioComponent } from './portfolio/create-portfolio/create-portfolio.component';


const routes: Routes = [
  { path: '', redirectTo: '/', pathMatch: 'full'},
  { path: 'test-grain', component: TestGrainComponent },
  { path: 'authorize', component: AuthorizeComponent },
  { path: 'authorize/token', component: TokenComponent },
  { path: 'portfolio', component: PortfolioComponent },
  { path: 'portfolio/create', component: CreatePortfolioComponent }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
