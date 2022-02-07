import { Component, NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AuthorizeComponent } from './authorize/authorize.component';
import { TestGrainComponent } from './test-grain/test-grain.component';
import { TokenComponent } from './authorize/token/token.component';


const routes: Routes = [
  { path: '', redirectTo: '/', pathMatch: 'full'},
  { path: 'test-grain', component: TestGrainComponent },
  { path: 'authorize', component: AuthorizeComponent },
  { path: 'authorize/token', component: TokenComponent }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
