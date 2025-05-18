import { NgModule } from "@angular/core";
import { AppComponent } from "./app.component";
import { BrowserModule } from "@angular/platform-browser";
import { RouterOutlet } from "@angular/router";
import { HomePageModule } from "./components/home-page/home-page.module";


@NgModule({
  declarations: [AppComponent],
  imports: [BrowserModule, RouterOutlet, HomePageModule],
  providers: [],
  bootstrap: [AppComponent]
})

export class AppModule { }