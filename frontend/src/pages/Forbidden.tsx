import { useAuthContext } from "../contexts/authContext";

const Forbidden = () => {
  const { decodedToken } = useAuthContext();
  const roleString =
    "http://schemas.microsoft.com/ws/2008/06/identity/claims/role";
  const homePage =
    (decodedToken as { [roleString]: string })[roleString] === "SuperUser"
      ? "/admin"
      : "/user";
  return (
    <div className="text-center">
      <h1 className="text-9xl font-extrabold text-red-500">403</h1>
      <p className="text-2xl font-semibold mt-4">Access Forbidden</p>
      <p className="mt-2 text-gray-600">
        You don't have permission to view this page.
      </p>
      <a
        href={homePage}
        className="inline-block mt-6 px-6 py-3 bg-red-500 text-white rounded-lg hover:bg-red-600 transition"
      >
        Go Home
      </a>
    </div>
  );
};
export default Forbidden;
