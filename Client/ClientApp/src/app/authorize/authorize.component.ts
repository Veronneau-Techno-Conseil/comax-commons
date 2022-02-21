import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder } from '@angular/forms'
import { AuthorizeServiceService } from './authorize-service.service'
import { AccountModel } from './Models/authorize-model';

@Component({
  selector: 'app-authorize',
  templateUrl: './authorize.component.html',
  styleUrls: ['./authorize.component.css']
})
export class AuthorizeComponent implements OnInit {

  AuthorizeForm!: FormGroup;
  AccountModel: AccountModel = new AccountModel();

  constructor(private fb: FormBuilder, private authorize: AuthorizeServiceService) { }

  ngOnInit(): void {
    this.InitializeForm();
  }

  onSubmit(): void {
    this.authorize.RequestAuthorization(this.AccountModel).subscribe();
  }

  InitializeForm(): void {
    this.AuthorizeForm = this.fb.group({
      ClientID: ''
    });
  }
}
