import { BookRequestsList } from "../components/BookRequestList";

const BookBorrowingPage = () => {
  return (
    <div className="p-6">
      <h1 className="text-2xl font-bold mb-6">Book Requests</h1>
      <BookRequestsList />
    </div>
  );
};
export default BookBorrowingPage;
