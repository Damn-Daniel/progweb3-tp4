import { Component, OnInit } from '@angular/core';
import { GalleryService } from '../services/gallery.service';

@Component({
  selector: 'app-publicGalleries',
  templateUrl: './publicGalleries.component.html',
  styleUrls: ['./publicGalleries.component.css']
})
export class PublicGalleriesComponent implements OnInit {

  constructor(public gallery : GalleryService) { }

  async ngOnInit() : Promise<void> {
    await this.gallery.getPublicGalleries();
  }

}
