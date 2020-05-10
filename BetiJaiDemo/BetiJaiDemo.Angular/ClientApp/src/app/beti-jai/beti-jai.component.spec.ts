import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { BetiJaiComponent } from './beti-jai.component';

describe('BetiJaiComponent', () => {
  let component: BetiJaiComponent;
  let fixture: ComponentFixture<BetiJaiComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ BetiJaiComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(BetiJaiComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
