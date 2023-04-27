import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../services/auth.service';
import { RegisterDTO } from '../models/registerDTO';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {

  username : string = "";
  email : string = "";
  password : string = "";
  passwordConfirm : string = "";

  constructor(public router : Router, public auth : AuthService) { }

  ngOnInit() {
  }

  async register(): Promise<void>{
    await this.auth.register(new RegisterDTO(this.username, this.email, this.password, this.passwordConfirm));
    // Aller vers la page de connexion
    this.router.navigate(['/login']);
  }
}
