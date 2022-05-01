import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder } from '@angular/forms'
import { ActivatedRoute } from '@angular/router';
import { PortfolioModel } from '../Models/portfolio-model';
import { PortfolioServiceService } from '../portfolio-service.service';
import { UUID } from 'angular2-uuid';

@Component({
  selector: 'app-create-portfolio',
  templateUrl: './create-portfolio.component.html',
  styleUrls: ['./create-portfolio.component.css']
})
export class CreatePortfolioComponent implements OnInit {

  PortfolioForm!: FormGroup;
  PortfolioModel: PortfolioModel = new PortfolioModel();

  constructor(private fb: FormBuilder, private portfolio: PortfolioServiceService, private route: ActivatedRoute) { }

  ngOnInit(): void {
    this.InitializeForm();
    
  }

  onSubmit(): void {
    this.PortfolioModel.PortfolioID = UUID.UUID()
    this.portfolio.CreatePortfolio(this.PortfolioModel).subscribe(response => {
      console.log("Created");
    });

  }

  InitializeForm(): void {
    this.PortfolioForm = this.fb.group({
      PortfolioID: '',
      PortfolioName: ''
    });
  }

}
