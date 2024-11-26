import { ComponentFixture, TestBed } from '@angular/core/testing';


import { ManagementProductComponent } from './management-product.component';


describe('ManagementProductComponent', () => {
  let component: ManagementProductComponent;
  let fixture: ComponentFixture<ManagementProductComponent>;


  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ManagementProductComponent]
    })
      .compileComponents();


    fixture = TestBed.createComponent(ManagementProductComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });


  it('should create', () => {
    expect(component).toBeTruthy();
  });
});





