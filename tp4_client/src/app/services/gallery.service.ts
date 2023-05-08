import { HttpClient } from '@angular/common/http';
import { Injectable, ElementRef } from '@angular/core';
import { lastValueFrom } from 'rxjs';
import { Gallery } from '../models/gallery';
import { Picture } from '../models/picture';

const domain = "https://localhost:7217/";

@Injectable({
  providedIn: 'root'
})
export class GalleryService {

  // Galeries publiques (Galeries des autres utilisateurs)
  publicGalleries: Gallery[] = [];

  // Galeries privées (Galeries de l'utilisateur authentifié)
  myGalleries: Gallery[] = [];

  constructor(public http: HttpClient) { }

  // Obtenir les galeries des autres utilisateurs
  async getPublicGalleries(): Promise<void> {
    let x = await lastValueFrom(this.http.get<Gallery[]>(domain + "api/Galleries/GetPublicGalleries"));
    console.log(x);
    this.publicGalleries = x;
  }

  // Obtenir ses propres galeries
  async getMyGalleries(): Promise<void> {
    let x = await lastValueFrom(this.http.get<any>(domain + "api/Galleries/GetMyGalleries"));
    console.log(x);
    this.myGalleries = x;
  }

  // Créer une nouvelle galerie
  async postGallery(formData: FormData): Promise<void> {
    let x = await lastValueFrom(this.http.post<any>(domain + "api/Galleries/PostGallery", formData));
    console.log(x);
    this.myGalleries.push(x);
    window.location.reload();
  }

  // Ajouter une photo à une gallerie
  async postPicture(idGal: number, formdata: FormData): Promise<Picture> {
    let x = await lastValueFrom(this.http.post<any>(domain + "api/Pictures/PostPicture/" + idGal, formdata));
    console.log(x);
    return new Picture(x);
  }

  // Supprimer une de nos galeries à partir de son id
  async deleteGallery(id: number): Promise<void> {
    let x = await lastValueFrom(this.http.delete<any>(domain + "api/Galleries/DeleteGallery/" + id));
    console.log(x);
    for (let i = this.myGalleries.length - 1; i >= 0; i--) {
      if (this.myGalleries[i].id == id) {
        this.myGalleries.splice(i, 1);
      }
    }
    window.location.reload();
  }
  async deletePicture(id: number): Promise<void> {
    let x = await lastValueFrom(this.http.delete<any>(domain + "api/Pictures/DeletePicture/" + id));
    console.log(x);
  }


}
