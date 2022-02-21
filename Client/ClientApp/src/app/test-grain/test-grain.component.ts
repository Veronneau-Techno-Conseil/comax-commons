import { Component, Inject, OnInit } from '@angular/core';
import { SharedService } from '../shared.service';

@Component({
  selector: 'app-test-grain',
  templateUrl: './test-grain.component.html',
  styleUrls: ['./test-grain.component.css'],
})

export class TestGrainComponent implements OnInit {

  constructor(private service:SharedService) { }

  ngOnInit(): void {

  }
}
