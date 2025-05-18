import { NgModule } from "@angular/core";
import { HeaderComponent, } from "./header/header.component";
import { RouterModule, Routes } from "@angular/router";
import { HomePageComponent } from "./home-page.component";

const routes: Routes = [
  {
    path: '',
    component: HomePageComponent
  }
];

@NgModule({
  declarations: [HomePageComponent, HeaderComponent],
  imports: [RouterModule.forChild(routes)],
  exports: [HomePageComponent]
})

export class HomePageModule { }