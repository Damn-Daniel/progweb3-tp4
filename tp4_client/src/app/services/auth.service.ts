import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { RegisterDTO } from '../models/registerDTO';
import { lastValueFrom } from 'rxjs';
import { LoginDTO } from '../models/loginDTO';
import { GalleryService } from './gallery.service';

const domain = "https://localhost:7217/";

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  constructor(public http : HttpClient, public gallery : GalleryService) { }

  async register(registerDTO : RegisterDTO): Promise<void>{
    let x = await lastValueFrom(this.http.post<any>(domain + "api/users/register", registerDTO));
    console.log(x);
  }

  async login(loginDTO : LoginDTO): Promise<void>{
    let x = await lastValueFrom(this.http.post<any>(domain + "api/users/login", loginDTO));
    console.log(x);
    localStorage.setItem("token", x.token);
  }

  logout(){
    localStorage.removeItem("token");
    this.gallery.myGalleries = [];
  }

}
