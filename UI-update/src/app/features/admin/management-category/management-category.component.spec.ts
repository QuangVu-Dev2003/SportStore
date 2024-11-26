import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ManagementCategoryComponent } from './management-category.component';

describe('ManagementCategoryComponent', () => {
  let component: ManagementCategoryComponent;
  let fixture: ComponentFixture<ManagementCategoryComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ManagementCategoryComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ManagementCategoryComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
