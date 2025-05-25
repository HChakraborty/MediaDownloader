import { DisplayCards } from "../display-cards/display-cards"
import SearchSection from "../search-section/search-section"


const HomePage = () => {
  return(
    <>
      <SearchSection />
      <div className="flex justify-center items-center">
          <DisplayCards />
      </div>
    </>
  )
}

export default HomePage