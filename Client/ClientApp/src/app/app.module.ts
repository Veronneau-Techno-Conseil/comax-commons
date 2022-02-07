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

@NgModule({
  declarations: [
    AppComponent,
    TestGrainComponent,
    MenuComponent,
    AuthorizeComponent,
    TokenComponent
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
