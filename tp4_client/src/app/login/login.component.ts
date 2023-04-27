import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../services/auth.service';
import { LoginDTO } from '../models/loginDTO';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit {

  username : string = "";
  password : string = "";

  constructor(public router : Router, public auth : AuthService) { }

  ngOnInit() {
  }

  async login(): Promise<void>{
    await this.auth.login(new LoginDTO(this.username, this.password));
    // Retourner à la page d'accueil
    this.router.navigate(['/publicGalleries']);
  }

}
