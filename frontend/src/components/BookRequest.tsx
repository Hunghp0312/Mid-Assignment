import { useState, useEffect } from "react";
import { format } from "date-fns";
import {
  Check,
  X,
  Clock,
  ChevronDown,
  ChevronUp,
  Mail,
  Phone,
} from "lucide-react";
import {
  approveBorrowing,
  getBorrowing,
  rejectBorrowing,
} from "../services/bookBorrowingService";
import { toast } from "react-toastify";
// Type definitions
export enum BookRequestStatus {
  Waiting,
  Approved,
  Rejected,
}
export interface BookRequestItem {
  id: string;
  bookId: string;
  title: string;
  author: string;
  quantity: number;
}

export interface BookRequest {
  id: string;
  requestorName: string;
  requestorEmail: string;
  requestorPhone?: string;
  notes?: string;
  status: BookRequestStatus;
  requestDate: Date;
  bookDetails: BookRequestItem[];
}
export function BookRequestsList() {
  const [requests, setRequests] = useState<BookRequest[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [expandedRequestId, setExpandedRequestId] = useState<string | null>(
    null
  );
  const [updatingStatus, setUpdatingStatus] = useState<string | null>(null);
  const [statusFilter, setStatusFilter] = useState<
    BookRequest["status"] | "all"
  >("all");
  //   const getBookRequests = async () => {};
  const mapStatusToString = (status: BookRequest["status"] | "all") => {
    switch (status) {
      case BookRequestStatus.Waiting:
        return "Waiting";
      case BookRequestStatus.Approved:
        return "Approved";
      case BookRequestStatus.Rejected:
        return "Rejected";
      default:
        return "";
    }
  };
  useEffect(() => {
    const fetchRequests = async () => {
      try {
        const data = await getBorrowing(mapStatusToString(statusFilter), 1, 10);
        console.log(data);
        setRequests(data.items);
      } catch (error) {
        console.error("Error fetching book requests:", error);
      } finally {
        setIsLoading(false);
      }
    };

    fetchRequests();
  }, [statusFilter]);

  const toggleExpand = (requestId: string) => {
    setExpandedRequestId(expandedRequestId === requestId ? null : requestId);
  };

  const handleApproveBorrowing = async (requestId: string) => {
    setUpdatingStatus(requestId);
    console.log(requestId);
    try {
      await approveBorrowing(requestId);
      setRequests(
        requests.map((req) =>
          req.id === requestId
            ? { ...req, status: BookRequestStatus.Approved }
            : req
        )
      );
      toast.success("Request approved.");
    } catch (error) {
      console.error("Error approving borrowing:", error);
    } finally {
      setUpdatingStatus(null);
    }
  };
  const handleRejectBorrowing = async (requestId: string) => {
    setUpdatingStatus(requestId);
    try {
      await rejectBorrowing(requestId);
      setRequests(
        requests.map((req) =>
          req.id === requestId
            ? { ...req, status: BookRequestStatus.Rejected }
            : req
        )
      );
      toast.success("Request rejected.");
    } catch (error) {
      console.error("Error approving borrowing:", error);
    } finally {
      setUpdatingStatus(null);
    }
  };

  const getStatusBadge = (status: BookRequest["status"]) => {
    const statusConfig = [
      {
        color: "bg-yellow-500/20 text-yellow-500",
        icon: <Clock className="h-4 w-4 mr-1" />,
        text: "Waiting",
      },
      {
        color: "bg-blue-500/20 text-blue-500",
        icon: <Check className="h-4 w-4 mr-1" />,
        text: "Approved",
      },
      {
        color: "bg-red-500/20 text-red-500",
        icon: <X className="h-4 w-4 mr-1" />,
        text: "Rejected",
      },
    ];
    console.log(status);
    const config = statusConfig[status];

    return (
      <span
        className={`inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium ${config.color}`}
      >
        {config.icon}
        {config.text}
      </span>
    );
  };

  const formatDate = (date: Date) => {
    console.log(date);
    return format(new Date(date), "dd/MM/yyyy");
  };

  if (isLoading) {
    return (
      <div className="bg-gray-900 rounded-lg shadow-lg p-6">
        <div className="animate-pulse space-y-4">
          <div className="h-6 bg-gray-800 rounded w-1/4"></div>
          <div className="space-y-2">
            {[1, 2, 3].map((i) => (
              <div key={i} className="h-24 bg-gray-800 rounded"></div>
            ))}
          </div>
        </div>
      </div>
    );
  }

  //   if (requests.length === 0) {
  //     return (
  //       <div className="bg-gray-900 rounded-lg shadow-lg p-6 text-center">
  //         <p className="text-gray-400">No book requests found.</p>
  //       </div>
  //     );
  //   }

  return (
    <div className="bg-gray-900 rounded-lg shadow-lg">
      <div className="p-6 border-b border-gray-800">
        <h2 className="text-xl font-semibold text-white">Book Requests</h2>
        <p className="text-gray-400 mt-1">Manage and view all book requests</p>
        <div className="flex items-center space-x-2">
          <button
            onClick={() => setStatusFilter(BookRequestStatus.Waiting)}
            className={`px-3 py-1.5 text-sm rounded-md flex items-center ${
              statusFilter === BookRequestStatus.Waiting
                ? "bg-yellow-500/20 text-yellow-500 border border-yellow-500/30"
                : "bg-gray-800 text-gray-400 hover:bg-gray-700 hover:text-white"
            }`}
          >
            <Clock className="h-4 w-4 mr-1.5" />
            Waiting
          </button>
          <button
            onClick={() => setStatusFilter(BookRequestStatus.Approved)}
            className={`px-3 py-1.5 text-sm rounded-md flex items-center ${
              statusFilter === BookRequestStatus.Approved
                ? "bg-blue-500/20 text-blue-500 border border-blue-500/30"
                : "bg-gray-800 text-gray-400 hover:bg-gray-700 hover:text-white"
            }`}
          >
            <Check className="h-4 w-4 mr-1.5" />
            Approved
          </button>
          <button
            onClick={() => setStatusFilter(BookRequestStatus.Rejected)}
            className={`px-3 py-1.5 text-sm rounded-md flex items-center ${
              statusFilter === BookRequestStatus.Rejected
                ? "bg-red-500/20 text-red-500 border border-red-500/30"
                : "bg-gray-800 text-gray-400 hover:bg-gray-700 hover:text-white"
            }`}
          >
            <X className="h-4 w-4 mr-1.5" />
            Rejected
          </button>
          {statusFilter !== "all" && (
            <button
              onClick={() => setStatusFilter("all")}
              className="px-3 py-1.5 text-sm rounded-md bg-gray-800 text-gray-400 hover:bg-gray-700 hover:text-white"
            >
              Show All
            </button>
          )}
        </div>
      </div>

      <div className="divide-y divide-gray-800">
        {requests.length === 0 && (
          <div className="p-6 text-center">
            <p className="text-gray-400">No requests found.</p>
          </div>
        )}
        {requests.map((request) => (
          <div key={request.id} className="p-6">
            <div className="flex flex-col md:flex-row md:items-center justify-between gap-4">
              <div>
                <div className="flex items-center gap-3">
                  <h3 className="text-lg font-medium text-white">
                    {request.requestorName}
                  </h3>
                  {getStatusBadge(request.status)}
                </div>
                <p className="text-sm text-gray-400 mt-1">
                  Requested on {formatDate(request.requestDate)}
                </p>
              </div>

              <div className="flex items-center gap-3">
                <div className="text-sm text-gray-400">
                  <span className="font-medium text-white">
                    {request.bookDetails.length}
                  </span>{" "}
                  books
                </div>

                <button
                  onClick={() => toggleExpand(request.id)}
                  className="p-1 rounded-md bg-gray-800 text-gray-400 hover:bg-gray-700 hover:text-white"
                  aria-label={
                    expandedRequestId === request.id
                      ? "Collapse details"
                      : "Expand details"
                  }
                >
                  {expandedRequestId === request.id ? (
                    <ChevronUp className="h-5 w-5" />
                  ) : (
                    <ChevronDown className="h-5 w-5" />
                  )}
                </button>
              </div>
            </div>

            {expandedRequestId === request.id && (
              <div className="mt-4 space-y-4">
                {/* Contact Information */}
                <div className="bg-gray-800 rounded-md p-4">
                  <h4 className="text-sm font-medium text-gray-300 mb-2">
                    Contact Information
                  </h4>
                  <div className="space-y-2">
                    <div className="flex items-center gap-2 text-sm">
                      <Mail className="h-4 w-4 text-gray-500" />
                      <a
                        href={`mailto:${request.requestorEmail}`}
                        className="text-gray-300 hover:text-white"
                      >
                        {request.requestorEmail}
                      </a>
                    </div>
                    {request.requestorPhone && (
                      <div className="flex items-center gap-2 text-sm">
                        <Phone className="h-4 w-4 text-gray-500" />
                        <a
                          href={`tel:${request.requestorPhone}`}
                          className="text-gray-300 hover:text-white"
                        >
                          {request.requestorPhone}
                        </a>
                      </div>
                    )}
                  </div>
                  {request.notes && (
                    <div className="mt-3 pt-3 border-t border-gray-700">
                      <h4 className="text-sm font-medium text-gray-300 mb-1">
                        Notes
                      </h4>
                      <p className="text-sm text-gray-400">{request.notes}</p>
                    </div>
                  )}
                </div>

                {/* Requested Books */}
                <div>
                  <h4 className="text-sm font-medium text-gray-300 mb-2">
                    Requested Books
                  </h4>
                  <div className="bg-gray-800 rounded-md overflow-hidden">
                    <table className="min-w-full divide-y divide-gray-700">
                      <thead className="bg-gray-750">
                        <tr>
                          <th
                            scope="col"
                            className="px-4 py-3 text-left text-xs font-medium text-gray-400 uppercase tracking-wider"
                          >
                            Book
                          </th>
                          <th
                            scope="col"
                            className="px-4 py-3 text-right text-xs font-medium text-gray-400 uppercase tracking-wider"
                          >
                            Quantity
                          </th>
                        </tr>
                      </thead>
                      <tbody className="divide-y divide-gray-700">
                        {request.bookDetails.map((item) => (
                          <tr key={item.id}>
                            <td className="px-4 py-3">
                              <div>
                                <p className="text-sm font-medium text-white">
                                  {item.title}
                                </p>
                                <p className="text-xs text-gray-400">
                                  {item.author}
                                </p>
                              </div>
                            </td>
                            <td className="px-4 py-3 text-right text-sm font-medium text-white">
                              {item.quantity}
                            </td>
                          </tr>
                        ))}
                      </tbody>
                    </table>
                  </div>
                </div>

                {/* Actions */}
                {request.status === BookRequestStatus.Waiting && (
                  <div className="flex justify-end gap-3 pt-2">
                    <button
                      onClick={() => handleRejectBorrowing(request.id)}
                      disabled={updatingStatus === request.id}
                      className={`px-3 py-1.5 text-sm rounded-md border border-red-900 bg-red-900/20 text-red-400 hover:bg-red-900/30 focus:outline-none focus:ring-1 focus:ring-red-900 ${
                        updatingStatus === request.id
                          ? "opacity-50 cursor-not-allowed"
                          : ""
                      }`}
                    >
                      Reject Request
                    </button>
                    <button
                      onClick={() => handleApproveBorrowing(request.id)}
                      disabled={updatingStatus === request.id}
                      className={`px-3 py-1.5 text-sm rounded-md bg-gray-700 text-white hover:bg-gray-600 focus:outline-none focus:ring-1 focus:ring-gray-500 ${
                        updatingStatus === request.id
                          ? "opacity-50 cursor-not-allowed"
                          : ""
                      }`}
                    >
                      {updatingStatus === request.id ? (
                        <span className="flex items-center">
                          <svg
                            className="animate-spin -ml-1 mr-2 h-4 w-4 text-white"
                            xmlns="http://www.w3.org/2000/svg"
                            fill="none"
                            viewBox="0 0 24 24"
                          >
                            <circle
                              className="opacity-25"
                              cx="12"
                              cy="12"
                              r="10"
                              stroke="currentColor"
                              strokeWidth="4"
                            ></circle>
                            <path
                              className="opacity-75"
                              fill="currentColor"
                              d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"
                            ></path>
                          </svg>
                          Updating...
                        </span>
                      ) : (
                        "Approve Request"
                      )}
                    </button>
                  </div>
                )}

                {/* {request.status === BookRequestStatus.Approved && (
                  <div className="flex justify-end gap-3 pt-2">
                    <button
                      onClick={() =>
                        handleStatusUpdate(request.id, "fulfilled")
                      }
                      disabled={updatingStatus === request.id}
                      className={`px-3 py-1.5 text-sm rounded-md bg-gray-700 text-white hover:bg-gray-600 focus:outline-none focus:ring-1 focus:ring-gray-500 ${
                        updatingStatus === request.id
                          ? "opacity-50 cursor-not-allowed"
                          : ""
                      }`}
                    >
                      {updatingStatus === request.id
                        ? "Updating..."
                        : "Mark as Fulfilled"}
                    </button>
                  </div>
                )} */}
              </div>
            )}
          </div>
        ))}
      </div>
    </div>
  );
}
