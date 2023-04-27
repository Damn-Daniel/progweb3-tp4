import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { GalleryService } from '../services/gallery.service';
import { Gallery } from '../models/gallery';

@Component({
  selector: 'app-myGalleries',
  templateUrl: './myGalleries.component.html',
  styleUrls: ['./myGalleries.component.css']
})
export class MyGalleriesComponent implements OnInit {

  @ViewChild("myPictureViewChild", {static:false}) pictureInput ?: ElementRef;
  selectedGallery : Gallery | null = null;
  newOwner : string = "";
  name : string = "";
  isPublic : boolean = false;

  constructor(public gallery : GalleryService) { }

  async ngOnInit() : Promise<void> {
    await this.gallery.getMyGalleries();
  }

  selectGallery(g : Gallery){
    this.selectedGallery = g;
  }

  newGallery(){
    // Toutes les galeries sont publiques (pour simplifier, comme ce n'est plus à gérer)

    if (this.pictureInput ==undefined){
      console.log("input html non changé");
      return;
    }
    let file = this.pictureInput.nativeElement.files[0];
    if(file==null){
      console.log("Input html ne contient aucune image");
      return;
    }
    let formData = new FormData();
    formData.append("monImage", file, file.name);
    this.gallery.postGallery(new Gallery(0, this.name));
  }

}
