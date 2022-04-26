import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { HttpClientModule } from '@angular/common/http';
import { AppComponent } from './app.component';
import { TestGrainComponent } from './test-grain/test-grain.component';
import { AppRoutingModule } from './app-routing.module';
import { MenuComponent } from './menu/menu.component';
import { AuthorizeComponent } from './authorize/authorize.component';
import { ReactiveFormsModule } from '@angular/forms';
import { TokenComponent } from './authorize/token/token.component';
import { PortfolioComponent } from './portfolio/portfolio.component';
import { CreatePortfolioComponent } from './portfolio/create-portfolio/create-portfolio.component';

@NgModule({
  declarations: [
    AppComponent,
    TestGrainComponent,
    MenuComponent,
    AuthorizeComponent,
    TokenComponent,
    PortfolioComponent,
    CreatePortfolioComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    HttpClientModule,
    ReactiveFormsModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
