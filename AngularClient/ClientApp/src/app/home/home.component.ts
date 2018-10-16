import { Component, OnInit } from '@angular/core';
import { AuthService } from '../services/auth.service';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
})

export class HomeComponent implements OnInit {
  userName: string;
  constructor(private authService: AuthService) {

  }

  ngOnInit() {
    var c = this.authService.getUser();    
    this.userName = c.profile.name;
  }

  Logout() {
    this.authService.logout();
  }
}
