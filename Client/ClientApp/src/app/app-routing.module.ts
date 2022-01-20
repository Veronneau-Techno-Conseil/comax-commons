import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { TestGrainComponent } from './test-grain/test-grain.component';

const routes: Routes = [
  { path: '', redirectTo: '/', pathMatch: 'full'},
  { path: 'test-grain', component: TestGrainComponent }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
