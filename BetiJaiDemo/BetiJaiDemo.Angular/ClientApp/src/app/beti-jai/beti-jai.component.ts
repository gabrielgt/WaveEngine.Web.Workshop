import { Component, OnInit, ElementRef } from '@angular/core';
declare var App: any;

@Component({
  selector: 'app-beti-jai',
  templateUrl: './beti-jai.component.html',
  styleUrls: ['./beti-jai.component.css']
})
export class BetiJaiComponent implements OnInit {

  constructor() { }

  ngOnInit() {
    App.configure('wave-engine-canvas', 'BetiJaiDemo.Web', 'BetiJaiDemo.Web.Program');
    App.init();
  }

  goToInitialZone(initialZone: ElementRef) {
    App.displayZone(initialZone);
  }
}
