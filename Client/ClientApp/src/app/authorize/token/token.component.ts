import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder } from '@angular/forms';
import { AuthorizeServiceService } from '../authorize-service.service';
import { AccountModel } from '../Models/authorize-model';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-token',
  templateUrl: './token.component.html',
  styleUrls: ['./token.component.css']
})
export class TokenComponent implements OnInit {

  AccountForm!: FormGroup;
  AccountModel: AccountModel = new AccountModel;

  constructor(private fb: FormBuilder, private authorize: AuthorizeServiceService, private route: ActivatedRoute) { }

  ngOnInit(): void {
    this.InitializeForm();
    this.AccountModel.UserChallengeCode = this.route.snapshot.queryParamMap.get('code')!;
  }

  onSubmit(): void {
    // this.authorize.GetToken(this.AccountModel).subscribe(response => {
    //   this.AccountModel.AccessToken = response.access_token;
    //   this.authorize.SetDetails(this.AccountModel).subscribe();
    // });
  }

  InitializeForm(): void {
    this.AccountForm = this.fb.group({
      ClientID: '',
      ClientSecret: '',
      UserChallengeCode: ''
    });
  }
}
