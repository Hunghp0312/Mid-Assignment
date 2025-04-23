const Home = () => {
  return (
    <div className="p-6">
      <h1 className="text-2xl font-bold mb-4">Dashboard</h1>
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
        <div className="bg-gray-800 text-gray-100 rounded-lg shadow p-4">
          <h2 className="font-semibold mb-2">Total Users</h2>
          <p className="text-2xl font-bold">1,234</p>
        </div>
        <div className="bg-gray-800 text-gray-100 rounded-lg shadow p-4">
          <h2 className="font-semibold mb-2">Revenue</h2>
          <p className="text-2xl font-bold">$12,345</p>
        </div>
        <div className="bg-gray-800 text-gray-100 rounded-lg shadow p-4">
          <h2 className="font-semibold mb-2">Active Projects</h2>
          <p className="text-2xl font-bold">42</p>
        </div>
      </div>
      <div className="mt-6 bg-gray-800 text-gray-100 rounded-lg shadow p-4">
        <h2 className="font-semibold mb-4">Recent Activity</h2>
        <div className="space-y-3">
          {[1, 2, 3, 4, 5].map((item) => (
            <div key={item} className="border-b border-gray-700 pb-2">
              <p className="text-sm text-gray-300">
                User updated project #{item}
              </p>
              <p className="text-xs text-gray-500">2 hours ago</p>
            </div>
          ))}
        </div>
      </div>
    </div>
  );
};
export default Home;
