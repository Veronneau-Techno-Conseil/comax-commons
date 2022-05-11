import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder } from '@angular/forms'
import { AuthorizeServiceService } from './authorize-service.service'
import { AccountModel } from './Models/authorize-model';
import { AuthApi, LOGIN, OK } from '../contracts/AuthSteps';
@Component({
  selector: 'app-authorize',
  templateUrl: './authorize.component.html',
  styleUrls: ['./authorize.component.css']
})
export class AuthorizeComponent implements OnInit {

  AuthorizeForm!: FormGroup;
  AccountModel: AccountModel = new AccountModel();
  Mode?: string;
  constructor(private fb: FormBuilder, private authorize: AuthorizeServiceService) { }

  ngOnInit(): void {
    this.InitializeForm();
    this.authorize.Init().subscribe(res=>{
        this.Mode = res.result;
    })
    
  }

  onSubmit(): void {
    if(this.Mode == LOGIN){
      this.authorize.AuthenticateUser().subscribe();
    }
    else{
      this.authorize.AuthenticateCluster(this.AccountModel).subscribe();
    }
  }

  InitializeForm(): void {
    this.AuthorizeForm = this.fb.group({
      ClientID: '',
      ClientSecret:''
    });
  }
}
