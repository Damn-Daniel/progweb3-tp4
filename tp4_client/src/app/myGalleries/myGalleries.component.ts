import { Component, ElementRef, OnInit, QueryList, ViewChild, ViewChildren } from '@angular/core';
import { GalleryService } from '../services/gallery.service';
import { Gallery } from '../models/gallery';
import { Picture } from '../models/picture';
declare var Masonry: any;
declare var imagesLoaded: any;

@Component({
  selector: 'app-myGalleries',
  templateUrl: './myGalleries.component.html',
  styleUrls: ['./myGalleries.component.css']
})
export class MyGalleriesComponent implements OnInit {

  @ViewChild('masongrid') masongrid?: ElementRef;
  @ViewChildren('masongriditems') masongriditems?: QueryList<any>;
  @ViewChild("myPictureViewChild", { static: false }) pictureInput?: ElementRef;
  @ViewChild("pictureAdd", { static: false }) pictureInputAdd?: ElementRef;
  //@ViewChild("myPictureViewChild", { static: false }) pictureInput?: ElementRef;
  selectedGallery: Gallery | null = null;
  selectedPicture: Picture | null = null;
  newOwner: string = "";
  name: string = "";
  isPublic: boolean = false;

  constructor(public gallery: GalleryService) { }

  async ngOnInit(): Promise<void> {
    await this.gallery.getMyGalleries();
  }

  selectGallery(g: Gallery) {
    this.selectedGallery = g;
    this.unselectPicture();
  }
  selectPicture(p: Picture) {
    this.selectedPicture = p;
  }
  unselectPicture() {
    this.selectedPicture = null;
  }

  newGallery() {
    let formData = new FormData();
    if (this.pictureInput == undefined) {
      console.log("input html non changé");
      return;
    }
    let file = this.pictureInput.nativeElement.files[0];
    if (file == null) {
      console.log("Input html ne contient aucune image");
    }
    else {
      formData.append("monImage", file, file.name);
    }
    // Toutes les galeries sont publiques (pour simplifier, comme ce n'est plus à gérer)

    formData.append("gallery", this.name);
    console.log(this.name);
    this.gallery.postGallery(formData);
  }
  async newPicture() {

    if (this.pictureInputAdd == undefined) {
      console.log("input html non changé");
      return;
    }
    let file = this.pictureInputAdd.nativeElement.files[0];
    if (file == null) {
      console.log("Input html ne contient aucune image");
      return;
    }
    let formData = new FormData();
    formData.append("monImage", file, file.name);
    await this.gallery.postPicture(this.selectedGallery!.id, formData);

    if (this.selectedGallery != null) {
      let idselect = this.selectedGallery.id;

      await this.gallery.getMyGalleries();
      let x = await this.gallery.myGalleries.find(item => item.id == idselect);
      if (x != undefined)
        await this.selectGallery(x);
    }
    this.pictureInputAdd.nativeElement.files = [];
  }

  async deletepic(id: number) {
    await this.gallery.deletePicture(id);

    if (this.selectedGallery != null) {
      let idselect = this.selectedGallery.id;

      await this.gallery.getMyGalleries();
      let x = await this.gallery.myGalleries.find(item => item.id == idselect);
      if (x != undefined)
        await this.selectGallery(x);
    }
  }

  ngAfterViewInit() {
    this.masongriditems?.changes.subscribe(e => {
      this.initMasonry();
    });

    if (this.masongriditems!.length > 0) {
      this.initMasonry();
    }
  }

  initMasonry() {
    var grid = this.masongrid?.nativeElement;
    var msnry = new Masonry(grid, {
      itemSelector: '.grid-item',
      columnWidth: 200, // À modifier si le résultat est moche
      gutter: 3
    });

    imagesLoaded(grid).on('progress', function () {
      msnry.layout();
    });
  }


}


