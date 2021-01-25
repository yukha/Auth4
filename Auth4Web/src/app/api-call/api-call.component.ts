import { Component, OnInit } from '@angular/core';
import { ApiService } from '../api.service';
import { UserData } from '../user-data';

@Component({
  selector: 'app-api-call',
  templateUrl: './api-call.component.html',
  styleUrls: ['./api-call.component.scss']
})
export class ApiCallComponent implements OnInit {

  userData: UserData

  constructor(api: ApiService) { 
    api.getUserData().subscribe((data) => this.userData = data);
  }

  ngOnInit(): void {
  }

}
